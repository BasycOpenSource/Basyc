namespace Basyc.Shared.Models;

public class AddressModel
{
	public AddressModel(string country, string region, string district, string town, string street, int postCode, string houseNumber)
	{
		Country = country;
		Region = region;
		District = district;
		Town = town;
		Street = street;
		PostCode = postCode;
		HouseNumber = houseNumber;
	}

	public string Country { get; set; }
	public string Region { get; set; }
	public string District { get; set; }
	public string Town { get; set; }
	public string Street { get; set; }
	public int PostCode { get; set; }
	public string HouseNumber { get; set; }
}
