namespace Hotfix.Asset.Model;

public readonly record struct AssetTransferContext(
    string SourceContainerType,
    string SourceContainerId,
    string Reason)
{
    public static AssetTransferContext FromMailAttachment(long mailId)
    {
        return new AssetTransferContext("MailAttachment", mailId.ToString(), "MailAttachmentClaim");
    }
}
