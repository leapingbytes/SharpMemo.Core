FROM microsoft/dotnet:2.2-sdk as sdk

RUN dotnet tool install -g dotnet-warp

RUN mkdir /build
COPY Domain Persistence Rest Web appsettings.json *.cs SharpMemoServer.csproj /build/
WORKDIR /build
RUN dotnet restore
RUN /root/.dotnet/tools/dotnet-warp 

FROM microsoft/dotnet:2.2-runtime
WORKDIR /root/
RUN mkdir /root/UI
COPY obj/UI/* /root/UI/
COPY --from=sdk  /build/SharpMemoServer .
ENV STATIC_CONTENT_ROOT=/root/UI

CMD ["./SharpMemoServer"]  


