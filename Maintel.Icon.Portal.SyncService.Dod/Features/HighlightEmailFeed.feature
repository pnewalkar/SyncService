Feature: HighlightEmailFeed
	In order to parse Email information sent by highlight
	As a sync service
	I want to be able to parse and store these emails into a database

@highlight @email
Scenario: Success connect to email provider
	Given I have a "valid" username and "valid" password
	When I connect to my email provider
	Then a connection to the email provider "is" made

@highlight @email
Scenario Outline: Fail connect to email provider invalid credentials
	Given I have a "<usernameStatus>" username and "<passwordStatus>" password
	When I connect to my email provider
	Then a connection to the email provider "is not" made
	Examples: 
	| usernameStatus | passwordStatus |
	| invalid        | invalid        |
	| invalid        | valid          |
	| valid          | invalid        |



@highlight @email
Scenario Outline: Success retrieve emails
	Given I have a valid connection to my email provider
		And there "<number>" emails recieved from a "valid" email address
	When I try to recieve emails
	Then "<number>" new emails are receieved
Examples: 
| number |
| 1      |
| 8     |
| 17     |

@highlight @email
Scenario Outline: Fail retrieve emails
	Given I have a valid connection to my email provider
		And there "<number>" emails recieved from a "invalid" email address
	When I try to recieve emails
	Then "0" new emails are receieved
Examples: 
| number |
| 1      |
| 10     |


@highlight @email
Scenario Outline: Success and Fail retrieve emails
	Given I have a valid connection to my email provider
		And there "<numberValid>" emails recieved from a "valid" email address
		And there "<numberInvalid>" emails recieved from a "invalid" email address
	When I try to recieve emails
	Then "<numberValid>" new emails are receieved
Examples: 
| numberValid | numberInvalid |
| 1           | 9             |
| 10          | 10            |
| 9           | 1             |


@highlight @email
Scenario Outline: Success Download and Add Emails to database
	Given I have a valid connection to my email provider
		And I have a valid connection to my database
		And there "<numberValid>" emails recieved from a "valid" email address
		And there "<numberInvalid>" emails recieved from a "invalid" email address
	When I run the receive add to database process
	Then "<numberValid>" new "emailalerts" are in the database
Examples: 
| numberValid | numberInvalid |
| 1           | 9             |
| 10          | 10            |
| 9           | 1             |


@highlight @email
Scenario Outline: Success Convert Emails to RatingsMarker
	Given I have a valid connection to my database
		And I have a "<number>" of "fullemail" objects
	When I run the email converstion process
		And I run the add to ratingsmarker process
	Then "<number>" new "ratingsmarkers" are in the database
Examples: 
| number |
| 1      |
| 10     |
| 9      |  