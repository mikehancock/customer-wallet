
Customer Wallet
===============

An simple account wallet for a customer, exposed over WebApi

<dl>
    <dt>Setup</dt>
    <dd></dd>
</dl>

Website Runs in IIS, so please set up a website to run on http://localhost:50170 in either local IIS or IISExpress

Storage is using Embedded RavenDB, so you will need to give read / write permission on the Data directory to the IIS worker process identity.

<dl>
    <dt>Usage</dt>
    <dd></dd>
</dl>
```C#
var controllerUri = new Uri("http://localhost:50170/api/Wallet");
var request = new JsonRequestBuilder().WithUri(controllerUri).WithBody(1);
var response = new JsonClient().Post(request);
```
<dl>
    <dt>Requirements</dt>
    <dd></dd>
</dl>
**User story 1**

As the registration system I want to open accounts for new customers so that they can start playing on my site

Acceptance criteria: I call a service with a user id, and if the creation is successful, I get an account id back, if there is a failure, I get informed of the reason.

**User Story 2**

As a player I can see the balance of my account so that I know how much I can play

Acceptance criteria: Call a service with account ID, return the balance of the account. If unsuccessful i get informed of the reason.

**User Story 3**

As a player, I can withdraw funds from my account so that I can play and get my winnings.

Acceptance criteria: Call a service with account id and amount, if the withdrawal is successful, I get the new balance of the account. If unsuccessful i get informed of the reason.

**User Story 4**

As a player, I can deposit money into my account so that I can play games.

Acceptance criteria: Call a service with account id and amount, if the deposit is successful, I get the new balance of the account. If unsuccessful i get informed of the reason.

**User Story 5**

As the management system I want to close existing customer accounts so that the customer will not be able to play.

Acceptance criteria: I call a service, and if the closing is unsuccessful, i get informed of the reason. An account cannot be closed if it has money on it or is already closed. 
