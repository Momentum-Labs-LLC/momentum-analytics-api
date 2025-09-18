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
      EXPORT_BUCKET  = data.aws_s3_bucket.this-bucket.bucket
      #HOURS_LOOKBACK = 24
      #TRIM_TO_HOUR   = "true"
      PAGE_VIEWS_TABLE_ARN = data.aws_dynamodb_table.this-page-views.arn
      COLLECTED_PII_TABLE_ARN = data.aws_dynamodb_table.this-collected-pii.arn
      PII_VALUES_TABLE_ARN = data.aws_dynamodb_table.this-pii.arn
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