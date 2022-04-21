FROM mcr.microsoft.com/dotnet/sdk AS base
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
  jq \
  mono-complete \
  nuget \
  tree \
  unzip \
  vim \
  && rm -rf /var/lib/apt/lists/*
COPY .bee/desperatedevs/.bashrc /root/.bashrc
RUN dotnet tool install -g dotnet-reportgenerator-globaltool \
  && dotnet tool install -g coverlet.console \
  && echo 'PATH="$HOME/.dotnet/tools:$PATH"' >> /root/.bashrc

FROM base AS bee
RUN bash -c "$(curl -fsSL https://raw.githubusercontent.com/sschmid/bee/main/install)" \
  && echo "complete -C bee bee" >> /root/.bashrc \
  && bee bee::run pull
WORKDIR /DesperateDevs
COPY Beefile Beefile
COPY Beefile.lock Beefile.lock
COPY .bee .bee
RUN bee bee::run install
