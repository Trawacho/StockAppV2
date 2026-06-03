FROM debian:bookworm

ARG NSIS_VERSION=3.08-2

ARG FirewallPlugin=https://nsis.sourceforge.io/mediawiki/images/e/e0/NSIS_Simple_Firewall_Plugin_Unicode_1.21.zip

ARG ServicePlugin=https://nsis.sourceforge.io/mediawiki/images/c/c9/NSIS_Simple_Service_Plugin_1.30.zip

RUN apt-get update && \
    apt-get install -y curl unzip nsis && \
    apt-get clean

RUN curl ${FirewallPlugin} -o /tmp/firewall_plugin.zip && \
    unzip /tmp/firewall_plugin.zip SimpleFC.dll -d /usr/share/nsis/Plugins/x86-ansi && \
    unzip /tmp/firewall_plugin.zip SimpleFC.dll -d /usr/share/nsis/Plugins/x86-unicode && \
    curl ${ServicePlugin} -o /tmp/service_plugin.zip && \
    unzip /tmp/service_plugin.zip SimpleSC.dll -d /usr/share/nsis/Plugins/x86-ansi && \
    unzip /tmp/service_plugin.zip SimpleSC.dll -d /usr/share/nsis/Plugins/x86-unicode && \
    rm /tmp/firewall_plugin.zip && \
    rm /tmp/service_plugin.zip

WORKDIR /build

ENTRYPOINT ["makensis", "-V4"]