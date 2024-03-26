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