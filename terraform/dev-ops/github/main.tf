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

module "iam_github_ecr_role" {
  source = "terraform-aws-modules/iam/aws//modules/iam-github-oidc-role"

  name = "${local.name_prefix}-github-role-${local.iteration}"

  # This should be updated to suit your organization, repository, references/branches, etc.
  subjects = [
    # specific repository
    "Momentum-Labs-LLC/momentum-analytics-api:*",
  ]

  policies = {
    ECRPolicy = aws_iam_policy.this-ecr-policy.arn
  }

  tags = local.tags
}

data "aws_ecr_repository" "this_ecr" {
  name = "${local.corp}-${local.project}-api-repo"
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
        "Resource" : data.aws_ecr_repository.this_ecr.arn
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