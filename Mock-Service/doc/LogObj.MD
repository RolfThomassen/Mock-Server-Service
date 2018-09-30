Objective for Rolf

1. Add environment variables LOG_LEVEL, ENVIRONMENT
2. Supported log levels
2.1. DEBUG
2.2. INFO - does not include FE polling, includes commands only
2.3. ERROR
2.4. OFF - enabled at the local host by default

Log format for Log

DEBUG
[StationMockService][STAGING] 198.54.233.154:50581 <= 172.24.6.17:3000 "FE"
[StationMockService][DEV] 198.54.233.154:50581 => 172.24.6.17:3000 "2802020202020202020;0?12020200000000000000000000000000000000000000"

INFO
[StationMockService][LOCAL] 198.54.233.154:50581 <= 172.24.6.17:3000 "M01"
[StationMockService][LOCAL] Pump #1: Idle => Reserved

[StationMockService][LOCAL] 198.54.233.154:50581 <= 172.24.6.17:3000 "A0101002500002000"
[StationMockService][LOCAL] Pump #1: Reserved => Authorized (10L or 50.00RM)

[StationMockService][LOCAL] Pump #1: NooseleUp => Dispensing

[StationMockService][LOCAL] 198.54.233.154:50581 <= 172.24.6.17:3000 "B01EHM$VPL"
[StationMockService][LOCAL] 198.54.233.154:50581 => 172.24.6.17:3000 "01H0M1$0000004558V0000020000P002279L002500["
[StationMockService][LOCAL] Pump #1: DispenseCompleted => Idle (3.5L or 45.30RM)