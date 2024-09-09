resource "aws_lambda_function" "this-function-0" {
  function_name = local.function_name

  package_type = "Image"
  image_uri    = data.aws_ecr_image.this-image.image_uri

  role = aws_iam_role.this-lambda-role.arn

  timeout                        = local.timeout
  memory_size                    = local.memory_size
  reserved_concurrent_executions = local.max_concurrency

  # vpc_config {
  #   security_group_ids = [aws_security_group.this-local-0.id, aws_security_group.this-self-0.id]
  #   subnet_ids         = data.aws_subnets.private.ids
  # }

  environment {
    variables = {
      FORCE_FAILURE = "false"
    }
  }

  tracing_config {
    mode = "PassThrough"
  }

  tags = merge(local.tags,
    tomap(
      {
        "Name" = local.function_name
      }
  ))
}

resource "aws_cloudwatch_log_group" "api-log-group" {
  name              = "/aws/lambda/${local.function_name}"
  retention_in_days = local.log_retention_days

  tags = merge(local.tags,
    tomap(
      {
        "Name" = local.function_name
      }
  ))
}

resource "aws_lambda_event_source_mapping" "this-event-source" {
  enabled                            = true
  event_source_arn                   = data.aws_dynamodb_table.this-collected-pii.stream_arn
  function_name                      = aws_lambda_function.this-function-0.arn
  starting_position                  = "LATEST"
  batch_size                         = local.db_batch_size
  maximum_batching_window_in_seconds = local.db_batching_window // wait n seconds to reach maximum batch size
  bisect_batch_on_function_error     = local.db_split_batch_on_error
  maximum_retry_attempts             = local.db_max_retries // dump the event after 100 attempts

  destination_config {
    on_failure {
      destination_arn = aws_sqs_queue.this-dlq.arn
    }
  }
}

resource "aws_lambda_event_source_mapping" "this-failure-retries" {
  enabled          = local.dlq_enabled
  event_source_arn = aws_sqs_queue.this-dlq.arn
  function_name    = aws_lambda_function.this-function-0.arn
  batch_size       = local.dlq_batch_size
}