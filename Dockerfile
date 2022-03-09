FROM mcr.microsoft.com/dotnet/sdk
RUN apt-get update && apt-get install -y --no-install-recommends \
  apt-transport-https \
  ca-certificates \
  dirmngr \
  gnupg \
  && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
  && echo "deb https://download.mono-project.com/repo/debian stable-buster main" | tee /etc/apt/sources.list.d/mono-official-stable.list \
  && apt-get update && apt-get upgrade -y \
  && rm -rf /var/lib/apt/lists/*
RUN apt-get update && apt-get install -y --fix-missing \
  mono-complete \
  nuget \
  && rm -rf /var/lib/apt/lists/*
RUN dotnet tool install -g dotnet-reportgenerator-globaltool \
  && echo 'PATH="$HOME/.dotnet/tools:$PATH"' >> ~/.bashrc
WORKDIR /app
COPY Beefile .
COPY .bee .bee
RUN bash -c "$(curl -fsSL https://raw.githubusercontent.com/sschmid/bee/main/install)" \
  && echo "complete -C bee bee" >> ~/.bashrc \
  && bee bee::run install
