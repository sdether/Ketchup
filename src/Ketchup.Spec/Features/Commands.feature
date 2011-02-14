Feature: Commands
	In order to store and retrieve data from a server
	As a developer
	I want to execute commands against memcached

@Add
Scenario: Set a new value
	Given a new Ketchup client
	When Set is executed with a key and a value
	Then the value should be added