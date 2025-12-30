# gRPC-Microservices in .NET

### This repository contains a solution with multiple microservices that communicate synchronously using gRPC. Unlike traditional REST APIs which often use JSON over HTTP/1.1, these services utilize HTTP/2 and binary serialization (Protobuf) for reduced latency and higher throughput.

## API Definition (Protobuf)
The API contract is defined in the .proto files located in the Protos directory.

Messages: Define the data structures (requests and responses).

Services: Define the RPC methods available for clients to call.

### To migrate database, individually cd in to ProductGrpc, DiscountGrpc, ShoppingCartGrpc and please run:

dotnet ef database update
