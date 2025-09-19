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
          "dynamodb:ExportTableToPointInTime",
          "dynamodb:DescribeTable"
        ],
        Effect = "Allow"
        Resource = [
          data.aws_dynamodb_table.this-page-views.arn,
          data.aws_dynamodb_table.this-collected-pii.arn,
          data.aws_dynamodb_table.this-pii.arn
        ]
      },
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
          "s3:PutObject",
          "s3:GetBucketLocation",
          "s3:ListBucket"
        ]
        Effect = "Allow"
        Resource = [
          data.aws_s3_bucket.this-export-bucket.arn,
          "${data.aws_s3_bucket.this-export-bucket.arn}/*",
        ]
      }
    ]
  })
}