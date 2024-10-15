## User Guide for AeroMetrics API

This API helps you store and retrieve telemetry data for race car performance analysis. Follow these instructions to use the API efficiently.

### API Endpoints

#### 1. Store Telemetry Data

- **Endpoint:** `/api/Performance/StoreTelemetryData`
- **Method:** POST
- **Description:** Stores telemetry data from a base64 encoded file string.
- **Consumes:** `application/json`, `text/json`, `application/*+json`
- **Parameters:**
  - **body:**
    - **Description:** The request containing the base64 encoded string of the file.
    - **Schema:**
      ```json
      {
        "fileContent": "string"
      }
      ```
- **Instructions:**
  - Convert your data file to a base64 encoded string before uploading.
  - Make sure to use the “Clear Telemetry Data” endpoint before uploading the same set of data again to avoid primary key issues.
- **Responses:**
  - **200:** Success

#### 2. Get Telemetry Data

- **Endpoint:** `/api/Performance/GetTelemetryData`
- **Method:** GET
- **Description:** Retrieves telemetry data analysis results.
- **Produces:** `text/plain`, `application/json`, `text/json`
- **Responses:**
  - **200:** Success
    - **Schema:**
      ```json
      [
        {
          "time": "double",
          "telemetry": [
            {
              "time": "double",
              "channel": "integer",
              "outing": "integer",
              "value": "double"
            }
          ]
        }
      ]
      ```

#### 3. Get Times for Default Conditions

- **Endpoint:** `/api/Performance/GetTimesForDefaultCondition`
- **Method:** GET
- **Description:** Retrieves the times for default conditions.
- **Produces:** `text/plain`, `application/json`, `text/json`
- **Responses:**
  - **200:** Success
    - **Schema:**
      ```json
      {
        "firstConditionTime": "double",
        "secondConditionTime": "double",
        "bothConditionTime": "double"
      }
      ```

#### 4. Clear Telemetry Data

- **Endpoint:** `/api/Performance/ClearTelemetryData`
- **Method:** DELETE
- **Description:** Deletes the telemetry data.
- **Responses:**
  - **200:** Success

### Additional Information

- **Swagger URL:** [AeroMetrics API Documentation](https://aeroperformanceanlayzerservice-d2bretahcsexckgf.canadacentral-01.azurewebsites.net/swagger/index.html)
- **Swagger Version:** 2.0
- **API Title:** AeroMetrics API
- **API Version:** v1
