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
  subproject  = "ecr"
  name_prefix = "${local.corp}-${local.project}"
  imageCount  = 5

  tags = {
    Production = "true"
    Project    = "${local.project}-${local.subproject}"
  }
}

resource "aws_ecr_repository" "this-api-repository" {
  name                 = "${local.name_prefix}-api-repo"
  image_tag_mutability = "MUTABLE"

  tags = merge(local.tags,
    tomap(
      {
        "Name" : "${local.name_prefix}-api-repo"
    })
  )
}

resource "aws_ecr_lifecycle_policy" "this-api-lifecycle" {
  repository = aws_ecr_repository.this-api-repository.name

  policy = jsonencode({
    "rules" : [
      {
        "rulePriority" : 1,
        "description" : "Keep last 5 images",
        "selection" : {
          "tagStatus" : "tagged",
          "tagPrefixList" : ["1.0"],
          "countType" : "imageCountMoreThan",
          "countNumber" : local.imageCount
        },
        "action" : {
          "type" : "expire"
        }
      }
    ]
  })
}

resource "aws_ecr_repository" "this-page-views-repository" {
  name                 = "${local.name_prefix}-page-views-processing-repo"
  image_tag_mutability = "MUTABLE"

  tags = merge(local.tags,
    tomap(
      {
        "Name" : "${local.name_prefix}-page-views-processing-repo"
    })
  )
}

resource "aws_ecr_lifecycle_policy" "this-page-views-lifecycle" {
  repository = aws_ecr_repository.this-page-views-repository.name

  policy = jsonencode({
    "rules" : [
      {
        "rulePriority" : 1,
        "description" : "Keep last 5 images",
        "selection" : {
          "tagStatus" : "tagged",
          "tagPrefixList" : ["1.0"],
          "countType" : "imageCountMoreThan",
          "countNumber" : local.imageCount
        },
        "action" : {
          "type" : "expire"
        }
      }
    ]
  })
}

resource "aws_ecr_repository" "this-pii-repository" {
  name                 = "${local.name_prefix}-pii-processing-repo"
  image_tag_mutability = "MUTABLE"

  tags = merge(local.tags,
    tomap(
      {
        "Name" : "${local.name_prefix}-pii-processing-repo"
    })
  )
}

resource "aws_ecr_lifecycle_policy" "this-pii-lifecycle" {
  repository = aws_ecr_repository.this-pii-repository.name

  policy = jsonencode({
    "rules" : [
      {
        "rulePriority" : 1,
        "description" : "Keep last 5 images",
        "selection" : {
          "tagStatus" : "tagged",
          "tagPrefixList" : ["1.0"],
          "countType" : "imageCountMoreThan",
          "countNumber" : local.imageCount
        },
        "action" : {
          "type" : "expire"
        }
      }
    ]
  })
}

resource "aws_ecr_repository" "this-visits-repository" {
  name                 = "${local.name_prefix}-visit-reporting-repo"
  image_tag_mutability = "MUTABLE"

  tags = merge(local.tags,
    tomap(
      {
        "Name" : "${local.name_prefix}-visit-reporting-repo"
    })
  )
}

resource "aws_ecr_lifecycle_policy" "this-visits-lifecycle" {
  repository = aws_ecr_repository.this-visits-repository.name

  policy = jsonencode({
    "rules" : [
      {
        "rulePriority" : 1,
        "description" : "Keep last 5 images",
        "selection" : {
          "tagStatus" : "tagged",
          "tagPrefixList" : ["1.0"],
          "countType" : "imageCountMoreThan",
          "countNumber" : local.imageCount
        },
        "action" : {
          "type" : "expire"
        }
      }
    ]
  })
}