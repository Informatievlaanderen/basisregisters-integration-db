CREATE SCHEMA IF NOT EXISTS integration_bosa;

CREATE TABLE IF NOT EXISTS integration_bosa.postal_crab_versions (postal_code varchar(4) PRIMARY KEY, version_timestamp TEXT NOT NULL);
CREATE TABLE IF NOT EXISTS integration_bosa.municipality_crab_versions (nis_code varchar(5) PRIMARY KEY, version_timestamp TEXT NOT NULL)
CREATE TABLE IF NOT EXISTS integration_bosa.streetname_crab_versions (streetname_persistent_local_id INT PRIMARY KEY, version_timestamp TEXT NOT NULL, created_on TEXT NOT NULL);
CREATE TABLE IF NOT EXISTS integration_bosa.address_crab_versions (address_persistent_local_id INT PRIMARY KEY, version_timestamp TEXT NOT NULL, created_on TEXT NOT NULL);
