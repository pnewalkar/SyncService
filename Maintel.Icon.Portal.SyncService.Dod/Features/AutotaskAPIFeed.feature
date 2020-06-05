Feature: AutotaskAPIFeed
	In order to update ticket and site information
	As a sync service
	I want to query the Autotask Web API to update this information


@autotask @api
Scenario: Success Retrieve Single Sites
	Given I have a connection to the autotask API
	When I request details on a site known to exist
	Then A "SiteModel" should have some results

@autotask @api
Scenario: Fail Retrieve Single Sites
	Given I have a connection to the autotask API
	When I request details on a site known not to exist
	Then A "SiteModel" should have some results
	
@autotask @api
Scenario: Success Retrieve Child Sites
	Given I have a connection to the autotask API
	When I request details on a site known to have children
	Then A "List<SiteModel>" should have some results

@autotask @api
Scenario: Success Retrieve No Child Sites
	Given I have a connection to the autotask API
	When I request details on a site known to not have children
	Then A "List<SiteModel>" should have no results

@autotask @api
Scenario: Success Retrieve New Sites
	Given I have a connection to the autotask API
	When I request details on new sites
		And I query based on the "recent past" 
	Then A "List<SiteModel>" should have some results

@autotask @api
Scenario: Fail Retrieve New Sites
	Given I have a connection to the autotask API
	When I request details on new sites
		And I query based on the "future" 
	Then A "List<SiteModel>" should have no results

@autotask @api
Scenario: Success Retrieve All Tickets By Account
	Given I have a connection to the autotask API
	When I request details on a site known to have tickets
	Then A "List<TicketModel>" should have some results

@autotask @api
Scenario: Fail Retrieve All Tickets By Account
	Given I have a connection to the autotask API
	When I request details on a site known not to have tickets
	Then A "List<TicketModel>" should have no results

@autotask @api
Scenario: Success Retrieve Updated Tickets By Account
	Given I have a connection to the autotask API
	When I request details on a site known to have tickets 
		And I query based on the "recent past" 
	Then A "List<TicketModel>" should have some results

@autotask @api
Scenario: Fail Retrieve Updated Tickets By Account With Old Tickets
	Given I have a connection to the autotask API
	When I request details on a site known to have tickets 
		And I query based on the "future" 
	Then A "List<TicketModel>" should have some results

	
@autotask @api
Scenario: Fail Retrieve Updated Tickets By Account With No Tickets
	Given I have a connection to the autotask API
	When I request details on a site known not to have tickets 
		And I query based on the "future" 
	Then A "List<TicketModel>" should have some results