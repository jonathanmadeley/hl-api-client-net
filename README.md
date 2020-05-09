[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub forks](https://img.shields.io/nuget/dt/HargreavesLansdown.API.svg?style=flat-square)](https://www.nuget.org/packages/HargreavesLansdown.API/)

# Hargreaves Lansdown API Client
A simple API client for pulling information about your ISA's from Hargreaves Lansdown written in C#. This project uses the web portal for HL's services and outputs the results in a computer-readable format.


## Introduction

This is a simple client library for Hargreaves Lansdown. No Open API exists for regular traders so the routes this client operates on are those of which the web portal operates on. 


## Authentication

HL operate on a two-phase login process, login-step-one, followed by a redirect to login-step-two.

A critical cookie (HLWEBsession) is required to be set - as far as I can tell, this is the only required cookie.

It is important to note that *your account will be locked if you enter invalid credentials too many times*.


## Motivation

The motivation for this client comes from the lack of an Open API provided by Hargreaves Lansdown for their regular customers. Automated scripts allow individuals and small traders alike to manage their investments with ease; it does seem rather odd not to offer an Open API with functionality like that of AJ Bell.

The main reason behind building this project is portfolio rebalancing at the end of each period.


## Contributions

I have only added limited functionality to this client library, to service the needs I personally have. Pull requests are accepted and encouraged!


## Licensing

This software is provided under a MIT license.
