# StockHistoryApi
Test assignment, upload csv from google finance and query statistics. InfluxDB driven.

Use /swagger to test endpoints and /uploader as a helper to upload files. See details about apikey security below.

Supports:dd
 * uploading stock market data in the CSV format (as exported by google finance, Date, Open, High, Low, Close, Volume) 
 for individual stock symbols, e.g. GOOG. Data can be uploaded continuously, i.e. more data can be uploaded for the same stock when it becomes available.
 N.B. Exactly same data point it ignored if already exists in db.
 * List available stocks with the most recent update timestamp 
 * Query individual stock symbol for the following stats: min, avg, max, median, 95th percentile for each price type (OHLC) 
 with an ability to specify a subset of price types that must be included in the result (e.g. if only open and close must be returned)
 The statistics should also be timestamped (i.e. statistics should be attributed with the most recent data point available).
 * Simple apikey-based authentication mechanism to allow for multiple clients to upload their data and being able to query only their data.
 Send header "Authorization: ApiKey 123000" with any number, this number works as client id.

 
Required InfluxDB https://docs.influxdata.com/influxdb/v1.2/introduction/getting_started/, connection can be setup via web.config 
or overriden via Azure web app application setting.

