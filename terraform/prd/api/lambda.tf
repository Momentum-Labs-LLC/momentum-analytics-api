resource "aws_lambda_function" "this-api-0" {
  function_name = local.api_name

  package_type = "Image"
  image_uri    = data.aws_ecr_image.this-image.image_uri

  role = aws_iam_role.this-lambda-role.arn

  timeout     = local.timeout
  memory_size = local.memory_size
  #reserved_concurrent_executions = local.max_concurrency #intentionally no reserved concurrency

  # vpc_config {
  #   security_group_ids = [aws_security_group.this-local-0.id, aws_security_group.this-self-0.id]
  #   subnet_ids         = data.aws_subnets.private.ids
  # }

  environment {
    variables = {
      CORS_ORIGINS  = "https://${data.aws_route53_zone.this-zone.name}",
      COOKIE_DOMAIN = "${data.aws_route53_zone.this-zone.name}",
      PAGE_VIEWS_TABLE = data.aws_dynamodb_table.this-page-views.name
      PAGE_VIEWS_V2_TABLE = data.aws_dynamodb_table.this-page-views-v2.name
      PII_VALUES_TABLE = data.aws_dynamodb_table.this-pii.name
      COLLECTED_PII_TABLE = data.aws_dynamodb_table.this-collected-pii.name
    }
  }

  tracing_config {
    mode = "PassThrough"
  }

  tags = merge(local.tags,
    tomap(
      {
        "Name" = local.api_name
      }
  ))
}

resource "aws_cloudwatch_log_group" "api-log-group" {
  name              = "/aws/lambda/${local.api_name}"
  retention_in_days = local.log_retention_days

  tags = merge(local.tags,
    tomap(
      {
        "Name" = local.api_name
      }
  ))
}