data "aws_sns_topic" "sns" {
  name = "${local.corp}-${local.env}-${local.project}-alerting"
}

resource "aws_cloudwatch_metric_alarm" "this-api-errors" {
  alarm_name = "${local.name_prefix}-errors"

  alarm_description = ">= 2% Api Error Rate"

  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = local.api_error_rate_periods
  threshold           = local.api_error_rate_threshold

  alarm_actions = [data.aws_sns_topic.sns.arn]
  ok_actions    = [data.aws_sns_topic.sns.arn]

  metric_query {
    id          = "r1"
    label       = "Error Rate"
    expression  = "(e1 + e2) / c1 * 100"
    period      = local.api_error_rate_period
    return_data = true
  }

  metric_query {
    id = "c1"

    metric {
      metric_name = "Count"
      namespace   = "AWS/ApiGateway"
      period      = local.api_error_rate_period
      stat        = "Sum"
      unit        = "Count"

      dimensions = {
        ApiName = aws_api_gateway_rest_api.this-api-0.name
      }
    }
  }

  metric_query {
    id = "e1"

    metric {
      metric_name = "4XXError"
      namespace   = "AWS/ApiGateway"
      period      = local.api_error_rate_period
      stat        = "Sum"
      unit        = "Count"

      dimensions = {
        ApiName = aws_api_gateway_rest_api.this-api-0.name
      }
    }
  }

  metric_query {
    id = "e2"

    metric {
      metric_name = "5XXError"
      namespace   = "AWS/ApiGateway"
      period      = local.api_error_rate_period
      stat        = "Sum"
      unit        = "Count"

      dimensions = {
        ApiName = aws_api_gateway_rest_api.this-api-0.name
      }
    }
  }

  tags = local.tags
}

resource "aws_cloudwatch_metric_alarm" "this-api-requests" {
  alarm_name = "${local.name_prefix}-request-count"

  alarm_description = "Too few requests"

  comparison_operator = "LessThanOrEqualToThreshold"
  evaluation_periods  = local.api_error_rate_periods
  threshold           = local.api_request_threshold
  treat_missing_data  = "breaching"

  alarm_actions = [data.aws_sns_topic.sns.arn]
  ok_actions    = [data.aws_sns_topic.sns.arn]

  metric_query {
    id          = "c1"
    return_data = true
    metric {
      metric_name = "Count"
      namespace   = "AWS/ApiGateway"
      period      = local.api_request_period
      stat        = "Sum"
      unit        = "Count"

      dimensions = {
        ApiName = aws_api_gateway_rest_api.this-api-0.name
      }
    }
  }

  tags = local.tags
}