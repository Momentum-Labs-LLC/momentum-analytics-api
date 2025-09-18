data "aws_iam_policy_document" "assume_role" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }
}

resource "aws_iam_role" "this-lambda-role" {
  name               = "${local.name_prefix}-role"
  assume_role_policy = data.aws_iam_policy_document.assume_role.json
}

resource "aws_iam_role_policy_attachment" "this-lambda-vpc-policy" {
  role       = aws_iam_role.this-lambda-role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
}

resource "aws_iam_role_policy" "this-lambda-dynamo" {
  name = "${local.name_prefix}-dynamo-policy"
  role = aws_iam_role.this-lambda-role.name

  # Terraform's "jsonencode" function converts a
  # Terraform expression result to valid JSON syntax.
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = [
          "dynamodb:BatchGetItem",
          "dynamodb:BatchWriteItem",
          "dynamodb:ConditionCheckItem",
          "dynamodb:GetItem",
          "dynamodb:PutItem",
          "dynamodb:Query",
          "dynamodb:Scan",
          "dynamodb:UpdateItem"
        ],
        Effect = "Allow"
        Resource = [
          data.aws_dynamodb_table.this-visits.arn,
          "${data.aws_dynamodb_table.this-visits.arn}/*",
        ]
      },
      {
        Action = [
          "dynamodb:Query",
          "dynamodb:Scan",
        ]
        Effect = "Allow"
        Resource = [
          data.aws_dynamodb_table.this-collected-pii.arn,
          "${data.aws_dynamodb_table.this-collected-pii.arn}/*",
          data.aws_dynamodb_table.this-pii.arn,
          "${data.aws_dynamodb_table.this-pii.arn}/*",
        ]
      }
    ]
  })
}

resource "aws_iam_role_policy" "this-lambda-s3" {
  name = "${local.name_prefix}-s3-policy"
  role = aws_iam_role.this-lambda-role.name

  # Terraform's "jsonencode" function converts a
  # Terraform expression result to valid JSON syntax.
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = [
          "s3:Put*",
        ]
        Effect = "Allow"
        Resource = [
          data.aws_s3_bucket.this-bucket.arn,
          "${data.aws_s3_bucket.this-bucket.arn}/*",
        ]
      }
    ]
  })
}