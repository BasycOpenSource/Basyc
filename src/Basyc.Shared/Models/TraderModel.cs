namespace Basyc.Shared.Models;

public class TraderModel
{
#pragma warning disable CS8618
    public TraderModel()
#pragma warning restore CS8618
    {
    }

    public TraderModel(string traderName, AddressModel address, ContactModel contact, string cin, string vatId)
    {
        TraderName = traderName;
        Address = address;
        Contact = contact;
        Cin = cin;
        VatId = vatId;
    }

    public string TraderName { get; }
    public AddressModel Address { get; }
    public ContactModel Contact { get; }

    /// <summary>
    ///     ICO in czech
    /// </summary>
    public string Cin { get; }

    /// <summary>
    ///     DIC in czech
    /// </summary>
    public string VatId { get; }
}
