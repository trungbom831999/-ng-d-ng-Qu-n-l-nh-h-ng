using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerRestaurant.DTO
{
    public class Bill
    {
        private int id;
        private DateTime? dateCheckIn;
        private DateTime? dateCheckOut;
        private int status;

        public int Id { get => id; set => id = value; }
        public DateTime? DateCheckIn { get => dateCheckIn; set => dateCheckIn = value; }
        public DateTime? DateCheckOut { get => dateCheckOut; set => dateCheckOut = value; }
        public int Status { get => status; set => status = value; }

        public Bill(int id, DateTime? checkIn, DateTime? checkOut, int status)
        {
            this.Id = id;
            this.DateCheckIn = checkIn;
            this.DateCheckOut = checkOut;
            this.Status = status;
        }

        public Bill(DataRow row)
        {
            this.Id = (int)row["id"];
            this.DateCheckIn = (DateTime?)row["dateCheckIn"];

            var dateCheckOutTest = row["dateCheckOut"];
            if (dateCheckOutTest.ToString() != "")
                this.DateCheckOut = (DateTime?)dateCheckOutTest;

            this.Status = (int)row["statusBill"];
        }
    }
}
