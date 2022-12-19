//using Bogus;
//using Kontrer.Shared.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Kontrer.Shared.Tests.FakeData
//{
//    public static class CustomerFakeData
//    {
//        public static List<CustomerModel> GetCustomersWithAccommodation(int count)
//        {
//            if (count == 0)
//            {
//                return new List<CustomerModel>();
//            }

//            var f = new Faker();

//            var range = Enumerable.Range(1, 10000);
//            var randomized = f.Random.Shuffle(range);
//            var unique = randomized.Take(count);
//            var enumerator = unique.GetEnumerator();

//            var customers = new Faker<CustomerModel>()
//                //.RuleFor(x => x.CustomerId, x => idCounter++)
//                .RuleFor(x => x.CustomerId, x =>
//                {
//                    enumerator.MoveNext();
//                    return enumerator.Current;
//                })
//                .RuleFor(x => x.Accomodations, (Faker faker, CustomerModel customer) =>
//                {
//                    var accos = AccommodationFakeData.GetAccommodationsWithoutCustomer(faker.Random.Int(0, 5));
//                    foreach (var acco in accos)
//                    {
//                        acco.Blueprint.CustomerId = customer;
//                        acco.Cost = CostFakeData.GetAccommodationCosts(1)[0];
//                    }

//                    return accos;

//                })
//                .RuleFor(x => x.Contact.Email, x => x.Person.Email)
//                .RuleFor(x => x.FirstName, x => x.Person.FirstName)
//                .RuleFor(x => x.LastName, x => x.Person.LastName)
//                //.RuleFor(x=>x.PhoneNumber,x=>new Random().Next(000000000,99999999))
//                .RuleFor(x => x.Contact.PhoneNumber, (x) => x.Random.Replace("#########"))
//                .Generate(count);
//            return customers;
//        }

//        public static List<CustomerModel> GetCustomersWithoutAccommodation(int count)
//        {
//            if (count == 0)
//            {
//                return new List<CustomerModel>();
//            }

//            var f = new Faker();

//            var range = Enumerable.Range(1, 10000);
//            var randomized = f.Random.Shuffle(range);
//            var unique = randomized.Take(count);
//            var enumerator = unique.GetEnumerator();

//            var customers = new Faker<CustomerModel>()
//                //.RuleFor(x => x.CustomerId, x => idCounter++)
//                .RuleFor(x => x.CustomerId, x =>
//                {
//                    enumerator.MoveNext();
//                    return enumerator.Current;
//                })
//                .RuleFor(x => x.Contact.Email, x => x.Person.Email)
//                .RuleFor(x => x.FirstName, x => x.Person.FirstName)
//                .RuleFor(x => x.LastName, x => x.Person.LastName)
//                //.RuleFor(x=>x.PhoneNumber,x=>new Random().Next(000000000,99999999))
//                .RuleFor(x => x.Contact.PhoneNumber, (x) => x.Random.Replace("#########"))
//                .Generate(count);
//            return customers;
//        }

//    }
//}