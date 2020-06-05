Feature: AutotaskDatabaseSeed
	In order to ensure the latest information can be retrived from the Autotask database
	As a sync service
	I want to be able to retrieve all and updated information 


@autotask @database
Scenario Outline: Success Initially request Database information
	Given I have a valid connection to the Autotask database
	When I request an initialisation of "<objectType>"
	Then A collection of "<objectType>" are returned
Examples: 
| objectType |
| sites      |
| tickets    |
| statustype |
| priority   |
| taskstatus |


@autotask @database
Scenario Outline: Success Request Updated Database information
	Given I have a valid connection to the Autotask database
	When I request an initialisation of "<objectType>"
		And I query based on the "far past"
	Then A collection of "<objectType>" are returned
Examples: 
| objectType |
| sites      |
| tickets    |


##Omited this test as it is very long running on the sandbox servers (45minutes+)
#@autotask @database
#Scenario Outline: Fail Request Updated Database information
#	Given I have a valid connection to the Autotask database
#	When I request an initialisation of "<objectType>"
#		And I query based on the "future"
#	Then An empty collection of "<objectType>" are returned
#Examples: 
#| objectType |
#| sites      |
#| tickets    |
