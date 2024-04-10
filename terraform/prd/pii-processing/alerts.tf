data "aws_sns_topic" "sns" {
  name = "${local.corp}-${local.env}-${local.project}-alerting"
}

resource "aws_cloudwatch_metric_alarm" "this-processor-behind" {
  alarm_name = "${local.name_prefix}-malfunction"

  alarm_description = "Page View Processor Falling Behind Or Not Being Invoked"

  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = local.behind_periods
  threshold           = local.behind_threshold
  treat_missing_data  = "breaching" # missing data indicates no invocations

  alarm_actions = [data.aws_sns_topic.sns.arn]
  ok_actions    = [data.aws_sns_topic.sns.arn]

  metric_query {
    id          = "m1"
    return_data = true

    metric {
      metric_name = "IteratorAge"
      namespace   = "AWS/Lambda"
      period      = local.behind_period
      stat        = "Average"
      unit        = "Milliseconds"

      dimensions = {
        FunctionName = aws_lambda_function.this-function-0.function_name
      }
    }
  }

  tags = local.tags
}