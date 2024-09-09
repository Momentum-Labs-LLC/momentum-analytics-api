resource "aws_sqs_queue" "this-dlq" {
  name                       = local.dlq_name
  delay_seconds              = local.dlq_delay              // message are delayed from first delivery for n seconds
  visibility_timeout_seconds = local.dlq_visibility_timeout // messages are unavailable for n seconds after they are read

  tags = merge(local.tags,
    tomap(
      {
        "Name" = local.dlq_name
      }
  ))
}