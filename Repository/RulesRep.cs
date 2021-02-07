﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RulesMicroservice.Repository
{
    public class RulesRep : IRulesRep
    {
        Uri baseAddress = new Uri("https://localhost:44302/api");   //Port No.

        HttpClient client;
        public RulesRep()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;

        }
        public List<RulesMsg> evaluateMinBalCurrent()
        {
            List<CurrentAccount> ls = new List<CurrentAccount>();
            var msg = new List<RulesMsg>();
            var a = new RulesMsg();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/Account/getCurrentAccountList").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ls = JsonConvert.DeserializeObject<List<CurrentAccount>>(data);
            }
            foreach (var v in ls)
            {
                if (v.CBal < 500)
                {
                    a.AccId = v.CAId;
                    a.AccBal = v.CBal;
                    a.Message = "Service Charge Applicable";
                    msg.Add(a);
                }
            }
            return msg;
        }

        public List<RulesMsg> evaluateMinBalSavings()
        {
            List<SavingsAccount> ls1 = new List<SavingsAccount>();
            var msg = new List<RulesMsg>();
            var a = new RulesMsg();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/Account/getSavingAccountList").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ls1 = JsonConvert.DeserializeObject<List<SavingsAccount>>(data);
            }
            foreach (var v in ls1)
            {
                if (v.savingsAccountbalance < 500)
                {
                    v.savingsAccountbalance = v.savingsAccountbalance - 100;
                    a.AccId = v.savingsAccountId;
                    a.AccBal = v.savingsAccountbalance;
                    //v.savingsAccountbalance = v.savingsAccountbalance - 100;
                    a.Message = "Service Charge Applicable";
                    msg.Add(a);
                }
            }
            return msg;
        }

        public ServiceCharge getServiceCharges()
        {
            ServiceCharge ob = new ServiceCharge();
            ob.serviceChargeApplicable = 100F;
            return ob;
        }


        public ruleStatus evaluateMinBal(dwacc value)
        {
            AccountMsg ob = new AccountMsg();
            ruleStatus ob1 = new ruleStatus();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/Account/getAccount/" + value.AccountId).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ob = JsonConvert.DeserializeObject<AccountMsg>(data);
                ob.AccBal = ob.AccBal - value.Balance;
                if (ob.AccBal < 500)
                    ob1.message = "Warning";
                
                else
                    ob1.message = "No Warning";
            }
            return ob1;
        }
    }
}
