namespace Basyc.Shared.Models;

public class CustomerModel
{
#pragma warning disable CS8618
	public CustomerModel()
#pragma warning restore CS8618
	{
	}

	public int CustomerId { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public ContactModel Contact { get; set; }
	public AddressModel Address { get; set; }
}
