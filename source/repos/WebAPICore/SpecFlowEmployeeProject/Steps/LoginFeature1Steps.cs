using System;
using TechTalk.SpecFlow;
using WebAPICore.Controllers;
using WebAPICore.Data;

namespace SpecFlowEmployeeProject.Steps
{
    [Binding]
    public class LoginFeature1Steps
    {
        private UserDetailsController _controller = null;
        private readonly EmployeeContext _context;
        string EmailID;

        [Given(@"Emp EmailID for fetching data")]
        public void GivenEmpEmailIDForFetchingData()
        {
            EmailID = "Vaibhav113@gmail.com";
        }
        
        [When(@"EmailID are passed")]
        public void WhenEmailIDArePassed()
        {
            _controller = new UserDetailsController(_context);
        }
        
        [Then(@"the result should be")]
        public void ThenTheResultShouldBe(Table table)
        {
            _controller.GetUserDetails(EmailID);
        }
    }
}
