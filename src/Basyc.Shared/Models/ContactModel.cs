using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Shared.Models;

public class ContactModel
{
	public ContactModel(string email, string phoneNumber, AddressModel address)
	{
		Email = email;
		PhoneNumber = phoneNumber;
		Address = address;
	}

	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public AddressModel Address { get; set; }
}