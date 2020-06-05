Feature: HighlightWebApiEmailFeed
	In order to parse Email information sent by highlight
	As a sync service
	I want to be able to parse and store these emails into a database


@highlight @email
Scenario Outline: Success retrieve emails from api
	Given I have a valid connection to my email api
		And there "<number>" emails recieved from a "valid" email address
	When I try to recieve emails from my email api
	Then "<number>" new emails are receieved from my email api
Examples: 
| number |
| 1      |
| 8     |
| 17     |

@highlight @email
Scenario Outline: Fail retrieve emails from api
	Given I have a valid connection to my email api
		And there "<number>" emails recieved from a "invalid" email address
	When I try to recieve emails from my email api
	Then "0" new emails are receieved from my email api
Examples: 
| number |
| 1      |
| 10     |
#
#
@highlight @email
Scenario Outline: Success and Fail retrieve emails from api
	Given I have a valid connection to my email api
		And there "<numberValid>" emails recieved from a "valid" email address
		And there "<numberInvalid>" emails recieved from a "invalid" email address
	When I try to recieve emails from my email api
	Then "<numberValid>" new emails are receieved from my email api
Examples: 
| numberValid | numberInvalid |
| 1           | 1             |
| 1           | 9             |
| 10          | 10            |
| 9           | 1             |
#
#
#@highlight @email
#Scenario Outline: Success Download and Add Emails to database
#	Given I have a valid connection to my email provider
#		And I have a valid connection to my database
#		And there "<numberValid>" emails recieved from a "valid" email address
#		And there "<numberInvalid>" emails recieved from a "invalid" email address
#	When I run the receive add to database process
#	Then "<numberValid>" new "emailalerts" are in the database
#Examples: 
#| numberValid | numberInvalid |
#| 1           | 9             |
#| 10          | 10            |
#| 9           | 1             |
#
#
#@highlight @email
#Scenario Outline: Success Convert Emails to RatingsMarker
#	Given I have a valid connection to my database
#		And I have a "<number>" of "fullemail" objects
#	When I run the email converstion process
#		And I run the add to ratingsmarker process
#	Then "<number>" new "ratingsmarkers" are in the database
#Examples: 
#| number |
#| 1      |
#| 10     |
#| 9      |  