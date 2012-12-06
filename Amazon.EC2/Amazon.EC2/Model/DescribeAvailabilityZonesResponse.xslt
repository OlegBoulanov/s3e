<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ec2="http://ec2.amazonaws.com/doc/2008-12-01/" exclude-result-prefixes="ec2">
    <xsl:output method="xml" omit-xml-declaration="no" indent="yes"/>
    <xsl:variable name="ns" select="'http://ec2.amazonaws.com/doc/2008-12-01/'"/>
    <xsl:template match="ec2:DescribeAvailabilityZonesResponse">
        <xsl:element name="DescribeAvailabilityZonesResponse" namespace="{$ns}">
            <xsl:element name="ResponseMetadata" namespace="{$ns}">
                <xsl:element name="RequestId" namespace="{$ns}">
                    <xsl:value-of select="ec2:requestId"/>
                </xsl:element>
            </xsl:element>
            <xsl:element name="DescribeAvailabilityZonesResult" namespace="{$ns}">
                <xsl:apply-templates select="ec2:availabilityZoneInfo"/>
            </xsl:element>
        </xsl:element>
    </xsl:template>
    <xsl:template match="ec2:availabilityZoneInfo">
        <xsl:apply-templates select="ec2:item"/>
    </xsl:template>
    <xsl:template match="ec2:item">
        <xsl:element name="AvailabilityZone" namespace="{$ns}">
            <xsl:element name="ZoneName" namespace="{$ns}">
                <xsl:value-of select="ec2:zoneName"/>
            </xsl:element>
            <xsl:element name="ZoneState" namespace="{$ns}">
                <xsl:value-of select="ec2:zoneState"/>
            </xsl:element>
        </xsl:element>
    </xsl:template>
</xsl:stylesheet>


