﻿using System.Collections.Generic;

namespace Server.DataAccess.Model
{
    public class User
    {
        public int UserId { get; set; }

        public int? AccountId { get; set; }

        public virtual Account Account { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CashAccount> CashAccounts { get; set; }

        public virtual ICollection<Cashflow> Cashflows { get; set; }

        public User()
        {
            CashAccounts = new HashSet<CashAccount>();
            Cashflows = new HashSet<Cashflow>();
        }
    }
}
