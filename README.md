# Introduction

Electro Scientific Industries has several product lines: Flex, Geode, MLCC, and Fusion just to name a few. This project’s main objective is to create an application that can be used to measure the quality and release readiness of Software across all ESI product groups. The app accomplishes this by implementing two categories of SW metrics: 

**ISO-9001 Metrics** which aims to maintain and improve the quality of products each quarter

**Release Readiness Metrics** which was created by the quality assurance team to measure various aspects of the software process with the main concern of determining whether or not the software builds are ready for release to the customers

It's important to note that while this code resembles the codebase of the API that's currently up and running on ESI's engineering server, you will need a machine that is connected to the MKS VPN in order to run the existing queries.
Also, keep in mind that this is the backend API portion of the application, meaning that a frontend Power Bi App is required in order to properly make use of the data that's being queried here!

The app queries data from DevOps in order to create the metrics that populate each report. The categories of used information include WorkItems, Builds or runs, Build Definitions, and commits. The app currently implements the aforementioned metrics using these data across the main ESI product groups: Flex, Fusion, Geode, and MLCC projects.

<kbd><img src="https://github.com/FluffyCrocodile/Storage/blob/88e7208a723686db66974830b4082f4eaab25c48/dia.JPG" width="500"></kbd>

The app queries data from Azure DevOps in order to create the metrics that populate each report. The categories of used information include WorkItems, Builds or runs, Build Definitions, commits, etc. The app currently implements the metrics using these data across the main ESI product groups: Flex, Fusion, Geode, and MLCC projects.

The frontend is a Power BI Application that integrates with this backend API, and this is where end-users interact with the metrics and the contained visuals within. The Power Bi App is what receives all pushed updates to the end-users, such as the refreshed data for the metrics over time, any changes to the report visuals, as well as any new metrics that are added.
The App can be accessed either via Power Bi Services, or following a direct link. The link is available both on the company SharePoint page, as well as the DevOps Wiki page for the project.

The application also supports a relational database which is up and running alongside this web api on the ESI build server. An external sqlite relational database stores build related data, such as build failures for the last 460 days (encapsulating at least the last 4 quarters from the current time), and last 30 builds for all build definitions. 
The database also stores custom data, such as the average code coverage for each month, which is calculated monthly and stored indefinitely going forward. 
