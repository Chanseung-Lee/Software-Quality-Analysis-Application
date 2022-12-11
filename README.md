# Introduction

This is the backend API portion of the Software Quality Analysis Application, the primary project that I've worked on during my internship at Electro Scientific Industries.

ESI has several product lines: Flex, Geode, MLCC, and Fusion just to name a few. This project’s main objective is to create an application that can be used to measure the quality and release readiness of Software across all ESI product groups. The app accomplishes this by implementing two categories of SW metrics: 

**ISO-9001 Metrics** which aims to maintain and improve the quality of products each quarter

**Release Readiness Metrics** which was created by the quality assurance team to measure various aspects of the software process with the main concern of determining whether or not the software builds are ready for release to the customers

<kbd><img src="https://github.com/FluffyCrocodile/Storage/blob/60d30499501be94032c1a93f205cdbdd86087feb/Metric.JPG" width="500"></kbd>

It's important to note that while this code resembles the codebase of the API that's currently up and running on ESI's engineering server, you will need a machine that is connected to the MKS VPN in order to run the existing queries.
Also, keep in mind that this is the backend API portion of the application, meaning that a frontend Power Bi App is required in order to properly make use of the data that's being queried here!

# Overview

The app queries data from DevOps in order to create the metrics that populate each report. The categories of used information include WorkItems, Builds or runs, Build Definitions, and commits. The app currently implements the aforementioned metrics using these data across the main ESI product groups: Flex, Fusion, Geode, and MLCC projects.

<kbd><img src="https://github.com/FluffyCrocodile/Storage/blob/88e7208a723686db66974830b4082f4eaab25c48/dia.JPG" width="600"></kbd>

The app queries data from Azure DevOps in order to create the metrics that populate each report. The categories of used information include WorkItems, Builds or runs, Build Definitions, commits, etc. The app currently implements the metrics using these data across the main ESI product groups: Flex, Fusion, Geode, and MLCC projects.

<kbd><img src="https://github.com/FluffyCrocodile/Storage/blob/a2ec69f83fb987c3e12dea967801a4421485e733/Summary.jpg" width="500"></kbd>

# Backend API & FrontEnd Power Bi App

The frontend is a Power BI Application that integrates with this backend API, and this is where end-users interact with the metrics and the contained visuals within. The Power Bi App is what receives all pushed updates to the end-users, such as the refreshed data for the metrics over time, any changes to the report visuals, as well as any new metrics that are added.

The App can be accessed either via Power Bi Services portal, or following a direct link. The link is available both on the company SharePoint page, as well as the DevOps Wiki page for the project.

The BackEnd API is where the necessary data are acquired from Azure DevOps tfs client using a variety of different queries (such as WIQL queries and Build Queries). The API is set to be up and running on the ESI company's engineering build server along with the Data Gateway that connects the API to any and all Power Bi reports.

<kbd><img src="https://github.com/FluffyCrocodile/Storage/blob/07ebdf5fa6acc8501147f475d6441b561d3b8509/fefefe.JPG" width="500"></kbd>

The frontend of the application is a Power Bi application composed of many distinct Power Bi reports. Power Bi is a microsoft application for data analysis and display. However, it's very possible on paper to create a fresh, new different set of reports (or any other data analysis software) to query from this API. On a similar note, the code for this backend API can be slightly altered to query newly desired fields from a different DevOps group.

# Relational SQLite Database

The application also supports a relational database which is up and running alongside this web api on the ESI build server. An external sqlite relational database stores build related data, such as build failures for the last 460 days (encapsulating at least the last 4 quarters from the current time), and last 30 builds for all build definitions. 
The database also stores custom data, such as the average code coverage for each month, which is calculated monthly and stored indefinitely going forward. 

<kbd><img src="https://github.com/FluffyCrocodile/Storage/blob/c73e8ab9714225f52ab895ffd3d5f38be2110061/rw.JPG" width="500"></kbd>

# Key Purpose & Conclusion

The application has not only provided a way for engineers at ESI who are part of each product line to assess the state of each project's software development according to the ISO-9001 Software Metric, but it also solidified itself as a tool that higher-up management members of the company could use to visualize all aspects of the software over time. The application is still up and running at ESI today, and widely being used by both the engineers and managers.
