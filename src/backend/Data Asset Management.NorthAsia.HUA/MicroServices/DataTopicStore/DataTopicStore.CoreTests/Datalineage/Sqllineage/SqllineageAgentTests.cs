using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataTopicStore.Core.Datalineage.Sqllineage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Datalineage.Sqllineage.Tests
{
    [TestClass()]
    public class SqllineageAgentTests
    {
        [TestMethod()]
        public async Task ExtractTablesTest()
        {
            SqllineageAgent sqllineageAgent = new SqllineageAgent();

            var sql = @"with bomcost as(
	select 
		move.profit_center profit_center, move.postng_date date, sum(move.quantity*pr.standard_price/pr.per) cost
	from 
		Trace_SAPPO.tb_sap_movement2 move
	left join Trace_Finance.std_price pr 
	on move.material=pr.material
	where move.movement_type='101' 
	group by move.profit_center, move.postng_date
),scrap as
(
	select move.profit_center  profit_center, move.postng_date date, sum(mrb.total_price ) amount
	FROM Trace_SAPPO.tb_sap_movement2 move 
	left join Trace_eMRB.tp_global_mrb_tool mrb on move.reference=mrb.traveler_id and mrb.material=move.material
	where move.movement_type='551'  and mrb.sub_disposition in ('Scrap (Have system record no physical)','Scrap (Normal)','Scrap by bacth (Have system record no physical)','Scrap by bacth (Normal)')
	group by move.profit_center, move.postng_date
)
SELECT `date`,`profit_center`,`scrap`,`amount`,`cost` from (
select 
	bomcost.date, bomcost.profit_center, scrap.amount/bomcost.cost scrap,scrap.amount,bomcost.cost 
from 
	bomcost left join scrap 
on bomcost.date=scrap.date and bomcost.profit_center=scrap.profit_center
) as tt
group by `date`,`profit_center`,`scrap`,`amount`,`cost`
order by `date` ,`profit_center`";


            var tableSql = Convert.ToBase64String(Encoding.UTF8.GetBytes($"sqllineage -d mysql -e \"{sql}\""));

            var columnSql = Convert.ToBase64String(Encoding.UTF8.GetBytes($"sqllineage -e \"{sql}\" -l column"));
            Assert.Fail();
        }
    }
}