using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Shared.Models;

public class TraderModel
{
    public TraderModel()
    {

    }

    public TraderModel(string traderName, AddressModel address, ContactModel contact, string CIN, string VATID)
    {
        TraderName = traderName;
        Address = address;
        Contact = contact;
        this.CIN = CIN;
        this.VATID = VATID;
    }

    public string TraderName { get; }
    public AddressModel Address { get; }
    public ContactModel Contact { get; }

    /// <summary>
    /// ICO in czech
    /// </summary>
    public string CIN { get; }
    /// <summary>
    /// DIC in czech
    /// </summary>
    public string VATID { get; }
}
