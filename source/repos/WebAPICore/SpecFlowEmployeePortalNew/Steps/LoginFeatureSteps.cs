using System;
using TechTalk.SpecFlow;
using WebAPICore.Controllers;
using WebAPICore.Data;

namespace SpecFlowEmployeePortalNew.Steps
{
    [Binding]
    public class LoginFeatureSteps
    {
        private UserDetailsController _controller = null;
        private readonly EmployeeContext _context;
        long ID;
        [Given(@"Emp EmailID for fetching data")]
        public void GivenEmpEmailIDForFetchingData()
        {
            ID = 12;
        }
        
        [When(@"EmailID are passed")]
        public void WhenEmailIDArePassed()
        {
            _controller = new UserDetailsController(_context);
        }
        
        [Then(@"the result should be")]
        public void ThenTheResultShouldBe(Table table)
        {
            _controller.GetEmployeeOrg(ID);
        }
    }
}
