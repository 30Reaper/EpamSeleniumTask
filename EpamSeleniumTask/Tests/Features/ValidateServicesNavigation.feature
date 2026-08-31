Feature: Validate Navigation to Services Section
  In order to verify navigation to Services category pages
  As an automated test
  I want to open Services menu, select a category and validate page title and related expertise section

  Scenario Outline: Navigate to a service category and verify page title and related expertise
	Given I open the Epam home page
	When I open Services menu and select "<category>"
	Then the page title contains "<category>"
	And the 'Our Related Expertise' section is displayed

	Examples:
	  | category        |
	  | Generative AI   |
	  | Responsible AI  |
