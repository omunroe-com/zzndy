/**
 * @author Andriy_Vynogradov
 */

var raw_ind = '   CompanyIncome = 0,        CompanyOperatingExpenses = 1,        CompanyCapitalExpenditures = 2,        CompanyRoyaltyTax = 3,        ompanyAfterTaxNetCashFlow = 4,        CompanyAfterTaxPayout = 5,        CompanyAfterTaxRiskReturnRatio = 6,        CompanyAfterTaxProfitabilityIndex = 7,        CompanyAfterTaxGovernmentTake = 8,        CompanyAfterTaxStateTake = 9,        ThirdPartyCashFlow = 10,        ThirdPartyPayout = 11,        hirdPartyRiskReturnRatio = 12,        ThirdPartyProfitabilityIndex = 13,        NOCCashFlow = 14,        NOCPayout = 15,        NOCRiskReturnRatio = 16,        NOCProfitabilityIndex = 17,        GovernmentCashFlow = 18,        GovernmentPayout = 19,        GovernmentRiskReturnRatio = 20,        overnmentProfitabilityIndex = 21,        CompanyCashFlowPerBOE = 22'
var raw_oth = 'AveragePrice                DevelopmentCapital                ExplorationCapital                GovernmentRateOfReturn                GroupRateOfReturn                NocRateOfReturn                OperatingExpenses                OtherCapital                ProducingLife                ProjectLife                UnitDevelopmentCapital                UnitExplorationCapital                UnitOperatingExpenses                UnitOtherCapital'

function filterfn(e)
{
	return e != '' && !e.match(/^(?:\d+|\s+|=)$/);
}

function toXmlCasing(e)
{
    return e.replace(/([a-z](?=[A-Z](?=[a-x])))/g, '$1-').toLowerCase().replace(/ /g, '-')
}

function toPropertyRef(e)
{
    return ['<', e, ' propertyId="???" unit="MM$"/>'].join('')
}

var text = [];

text.push(raw_ind
	.split(/ |,/)
	.filter(filterfn)
	.map(toXmlCasing)
	.map(toPropertyRef)
	.join('\n'))

text.push(raw_oth
	.split(/ |,/)
	.filter(filterfn)
	.map(toXmlCasing)
	.map(toPropertyRef)
	.join('\n'))
	
	
var summary = <>
                <property id="127001" name="Date" type="DateTime" />
                <property id="127002" name="Post Tax Cash Flow" type="Measurement" unit="MM$" />
                <property id="127003" name="Group Income" type="Measurement" unit="MM$" />
                <property id="127004" name="Group Government Payment" type="Measurement" unit="MM$" />
                <property id="127005" name="Group Operating Expense" type="Measurement" unit="MM$" />
                <property id="127006" name="Group Capital Expenditure" type="Measurement" unit="MM$" />
                <property id="127007" name="After Tax Cash Flow" type="Measurement" unit="MM$" />
			</>
			
var ind = <>
                <property id="129001" name="Discount Rate" type="Measurement" unit="%" />
                <property id="129002" name="Group Income" type="Measurement" unit="MM$" />
                <property id="129003" name="Group Opex" type="Measurement" unit="MM$" />
                <property id="129004" name="Group Capex" type="Measurement" unit="MM$" />
                <property id="129005" name="Group Government Payments" type="Measurement" unit="MM$" />
                <property id="129006" name="Group Cash Flow" type="Measurement" unit="MM$" />
                <property id="129007" name="Group Payout" type="Measurement" unit="yr" />
                <property id="129008" name="Group Profitability Index" type="Measurement" unit="MM$" />
                <property id="129009" name="Group Government Take" type="Measurement" unit="MM$" />
                <property id="129010" name="Government Cash Flow" type="Measurement" unit="MM$" />
			</>
			
function makeTupleRefs(tupleId, dateId, xml)
{
	var collection=[];
	var i=-1;
	while(++i<xml.length())
	{
	collection.push(['<', toXmlCasing(xml[i].@name), ' tupleId="127000" valuePropertyId="',xml[i].@id,'" datePropertyId="127001" unit="', xml[i].@unit ,'"/>' ].join(''));
	}
	console.log(collection.join('\n'));
	return collection.join('\n')
}



text.push(makeTupleRefs(127000, 127001, summary))
text.push(makeTupleRefs(129000, 129001, ind))

document.write('<textarea rows="30" cols="144">' + text + '</textarea>');
