variable "replication_destinations" {
  type = list(object({
    region     = string
    account_id = string
  }))
  default = [
    {
      region     = "us-east-1"
      account_id = "206311056731"
    }
  ] # Replace with your actual destinations
}