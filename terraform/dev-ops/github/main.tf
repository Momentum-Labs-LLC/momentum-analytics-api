provider "aws" {
  region = local.region
}

data "aws_availability_zones" "available" {}

locals {
  env         = "prd"
  corp        = "momentum"
  iteration   = 0
  region      = "us-east-1"
  project     = "analytics"
  subproject  = "api"
  name_prefix = "${local.corp}-${local.env}-${local.project}-${local.subproject}"

  tags = {
    Production = "true"
    Project    = "${local.project}-${local.subproject}"
  }
}

################################################################################
# GitHub OIDC Role
################################################################################
module "iam_github_oidc_provider" {
  source = "terraform-aws-modules/iam/aws//modules/iam-oidc-provider"

  url = "https://token.actions.githubusercontent.com"

  client_id_list = [
    "sts.amazonaws.com",
  ]
}

resource "aws_iam_role" "github_ecr_role" {
  name = "${local.name_prefix}-github-role-${local.iteration}"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Federated = module.iam_github_oidc_provider.arn
        }
        Action = "sts:AssumeRoleWithWebIdentity"
        Condition = {
          StringEquals = {
            "token.actions.githubusercontent.com:aud" = "sts.amazonaws.com"
          }
          StringLike = {
            "token.actions.githubusercontent.com:sub" = "repo:Momentum-Labs-LLC/momentum-analytics-api:*"
          }
        }
      }
    ]
  })

  tags = local.tags
}

resource "aws_iam_role_policy_attachment" "github_ecr_policy" {
  role       = aws_iam_role.github_ecr_role.name
  policy_arn = aws_iam_policy.this-ecr-policy.arn
}

data "aws_ecr_repository" "this_ecr" {
  name = "${local.corp}-${local.project}-api-repo"
}

data "aws_ecr_repository" "this_pg_ecr" {
  name = "${local.corp}-${local.project}-page-views-processing-repo"
}

data "aws_ecr_repository" "this_pii_ecr" {
  name = "${local.corp}-${local.project}-pii-processing-repo"
}

data "aws_ecr_repository" "this_visit_ecr" {
  name = "${local.corp}-${local.project}-visit-reporting-repo"
}

data "aws_ecr_repository" "this_export_ecr" {
  name = "${local.corp}-${local.project}-export-repo"
}

resource "aws_iam_policy" "this-ecr-policy" {
  policy = jsonencode({
    "Version" : "2012-10-17",
    "Statement" : [
      {
        "Effect" : "Allow",
        "Action" : [
          "ecr:CompleteLayerUpload",
          "ecr:UploadLayerPart",
          "ecr:InitiateLayerUpload",
          "ecr:BatchCheckLayerAvailability",
          "ecr:PutImage"
        ],
        "Resource" : [
          data.aws_ecr_repository.this_ecr.arn,
          data.aws_ecr_repository.this_pg_ecr.arn,
          data.aws_ecr_repository.this_pii_ecr.arn,
          data.aws_ecr_repository.this_visit_ecr.arn,
          data.aws_ecr_repository.this_export_ecr.arn
        ]
      },
      {
        "Effect" : "Allow",
        "Action" : [
          "ecr:GetAuthorizationToken"
        ],
        "Resource" : "*"
      }
    ]
  })
}