FROM microsoft/dotnet:2.2-sdk
WORKDIR /yuki

# copy csproj and restore as distinct layers
COPY Yuki/*.csproj ./
RUN dotnet restore

# copy and build everything else
COPY . ./
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "Yuki/out/Yuki.dll"]
