resource "aws_scheduler_schedule" "this-schedule" {
  name = "${local.name_prefix}-schedule"
  flexible_time_window {
    mode = "OFF"
  }
  schedule_expression          = "cron(30 0 * * ? *)" //30 minutes after midnight
  schedule_expression_timezone = "America/New_York"        // Eastern Time Zone
  state                        = "ENABLED"                 // DISABLED
  target {
    arn      = aws_lambda_function.this-function-0.arn
    role_arn = aws_iam_role.this-schedule-role.arn
  }
}

data "aws_iam_policy_document" "schedule_assume_role" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["scheduler.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }
}

resource "aws_iam_role" "this-schedule-role" {
  name               = "${local.name_prefix}-schedule-role"
  assume_role_policy = data.aws_iam_policy_document.schedule_assume_role.json
}

resource "aws_iam_role_policy" "this-schedule-lambda" {
  name = "${local.name_prefix}-schedule-lambda-policy"
  role = aws_iam_role.this-schedule-role.name

  # Terraform's "jsonencode" function converts a
  # Terraform expression result to valid JSON syntax.
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = [
          "lambda:Invoke*",
        ]
        Effect = "Allow"
        Resource = [
          aws_lambda_function.this-function-0.arn
        ]
      }
    ]
  })
}