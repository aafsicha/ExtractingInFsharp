using System;
using System.Linq;
using LiveCoding.Api.Controllers;
using LiveCoding.Domain;
using LiveCoding.Persistence;
using LiveCoding.Services;
using NFluent;
using Xunit;

namespace LiveCoding.Tests
{
    public class BookingShould
    {
        [Fact]
        public void Reserve_bar_when_at_least_60_percent_of_devs_are_available()
        {
            var indoorBarName = "Bar La Belle Equipe";
            var indoorBars = new[]
            {
                new Bar(indoorBarName, new Capacity(10), new[] { DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
            };
            var devData = new[]
            {
                new DevData { Name = "Alice", OnSite = new[] { Wednesday, Thursday, Friday } },
                new DevData { Name = "Bob", OnSite = new[] { Thursday } },
                new DevData { Name = "Chad", OnSite = new[] { Friday } },
                new DevData { Name = "Dan", OnSite = new[] { Wednesday, Thursday } },
                new DevData { Name = "Eve", OnSite = new[] { Thursday } },
            };

            var controller = BuildController(indoorBars, devData);
            controller.MakeBooking();
            var result = controller.Get().Single();

            Check.That(result.Date).IsEqualTo(Thursday);
            Check.That(result.Bar.Name).IsEqualTo(indoorBarName);
        }

        [Fact]
        public void Do_not_reserve_bar_when_only_50_percent_of_devs_are_available()
        {
            var indoorBars = new[]
            {
                new Bar("indoorBarName", new Capacity(10), new[] { DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
            };
            var developers = new[]
            {
                new DevData { Name = "Alice", OnSite = new[] { Wednesday, Friday } },
                new DevData { Name = "Bob", OnSite = new[] { Thursday } },
                new DevData { Name = "Chad", OnSite = new[] { Friday } },
                new DevData { Name = "Dan", OnSite = new[] { Wednesday } },
                new DevData { Name = "Eve", OnSite = new[] { Thursday } },
            };

            var controller = BuildController(indoorBars, developers);
            var success = controller.MakeBooking();

            Check.That(success).IsFalse();
        }

        [Fact]
        public void Reserve_bar_when_it_is_open()
        {
            var indoorBarName = "Bar La Belle Equipe";
            var indoorBars = new[]
            {
                new Bar(indoorBarName, new Capacity(10), new[] { DayOfWeek.Thursday }),
                new Bar("Le Sirius", new Capacity(10), new[] { DayOfWeek.Friday })
            };
            var developers = new[]
            {
                new DevData { Name = "Bob", OnSite = new[] { Thursday } },
                new DevData { Name = "Alice", OnSite = new[] { Thursday } }
            };

            var controller = BuildController(indoorBars, developers);
            controller.MakeBooking();
            var result = controller.Get().Single();

            Check.That(result.Date).IsEqualTo(Thursday);
            Check.That(result.Bar.Name).IsEqualTo(indoorBarName);
        }

        [Fact]
        public void Do_not_reserve_bar_when_it_is_closed()
        {
            var indoorBars = new[]
            {
                new Bar("La belle équipe", new Capacity(10), new[] { DayOfWeek.Thursday }),
                new Bar("Le Sirius", new Capacity(10), new[] { DayOfWeek.Friday })
            };
            var developers = new[]
            {
                new DevData { Name = "Bob", OnSite = new[] { Wednesday } },
                new DevData { Name = "Alice", OnSite = new[] { Wednesday } }
            };

            var controller = BuildController(indoorBars, developers);
            var success = controller.MakeBooking();

            Check.That(success).IsFalse();
        }

        [Fact]
        public void Choose_bar_that_has_enough_space()
        {
            var indoorBars = new[]
            {
                new Bar("La belle équipe", new Capacity(3), new[] { DayOfWeek.Thursday }),
            };
            var developers = new[]
            {
                new DevData { Name = "Bob", OnSite = new[] { Wednesday, Friday } },
                new DevData { Name = "Chad", OnSite = new[] { Wednesday } },
                new DevData { Name = "Dan", OnSite = new[] { Wednesday } },
                new DevData { Name = "Eve", OnSite = new[] { Wednesday } },
            };

            var controller = BuildController(indoorBars, developers);
            var success = controller.MakeBooking();

            Check.That(success).IsFalse();
        }

        private static BookingController BuildController(Bar[] barData, DevData[] devData)
        {
            var bookingRepository = new FakeBookingRepository();
            return new BookingController(new BookingService(new FakeBarRepository(barData),
                new FakeDevRepository(devData), bookingRepository), bookingRepository);
        }

        private static Bar ABar() => new(
            "Wallace Pub",
             new Capacity(10),
            new[] { DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday }
        );


        private static readonly DateTime Wednesday = new(2022, 05, 11);
        private static readonly DateTime Thursday = Wednesday.AddDays(1);
        private static readonly DateTime Friday = Wednesday.AddDays(2);
    }
}