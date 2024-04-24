data "aws_ecr_image" "this-image" {
  repository_name = "${local.corp}-${local.project}-${local.subproject}-repo"
  image_tag       = var.image_tag
}

data "aws_dynamodb_table" "this-visits" {
  name = "${local.corp}-${local.env}-visits"
}

data "aws_dynamodb_table" "this-pii" {
  name = "${local.corp}-${local.env}-pii-values"
}

data "aws_dynamodb_table" "this-collected-pii" {
  name = "${local.corp}-${local.env}-collected-pii"
}

data "aws_s3_bucket" "this-bucket" {
  bucket = "${local.corp}-${local.env}-identified-visits-0"
}