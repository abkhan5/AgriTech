namespace AgriTech.Services.Servicebus;

internal record ScheduledMessageRecords
{
    public ScheduledMessageRecords()
    {

    }
    public ScheduledMessageRecords(string messageId, long sequenceNumber)
    {
        MessageId = messageId;
        SequenceNumber = sequenceNumber;
    }
    public string MessageId { get; set; }
    public long SequenceNumber { get; set; }
}