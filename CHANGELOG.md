# [2.1.0](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v2.0.0...v2.1.0) (2024-04-08)


### Bug Fixes

* add padding to safestring ([907d4a0](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/907d4a048613ad405feba97ff3a08cca6f93cc4f))
* Zele matching + add Halen and Hasselt matching ([5dcd9b2](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/5dcd9b2a6b2ddb4b65ad11ba9853ddc0214b895b))


### Features

* add script to pack national registry ([d388837](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/d388837396b22853b66fd6d8003e1076d2f89256))
* municipality based index interpretation ([fd83217](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/fd83217f2c71347006b363f7354d20100b6b62f3))

# [2.0.0](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.4.4...v2.0.0) (2024-03-18)


### Features

* inwonersaantallen ([b56aeab](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/b56aeab25d3cf5e69e76e1f29e659ad98704c6ff))
* move to dotnet 8.0.2 ([cbb3b3c](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/cbb3b3c510440b95750bc10f41c6b1ed290212d6))


### BREAKING CHANGES

* move to dotnet 8.0.2

## [1.4.4](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.4.3...v1.4.4) (2024-02-28)


### Bug Fixes

* count view ([dc5fcfe](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/dc5fcfe7c8c32913b3b01cf6c82f8c6583809a0b))

## [1.4.3](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.4.2...v1.4.3) (2024-02-27)


### Bug Fixes

* **bump:** correct cd suspicious cases ([10f53d5](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/10f53d5902881aacc91b9e2bee4184d71e227c71))

## [1.4.2](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.4.1...v1.4.2) (2024-02-27)


### Bug Fixes

* **ci:** correct cd suspicious cases api ([4cbe6a1](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/4cbe6a1c9cf99c41f09c052001589fdd466e0ac9))

## [1.4.1](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.4.0...v1.4.1) (2024-02-27)


### Bug Fixes

* **bump:** add suspicious cases cd ([59b746c](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/59b746c463c90147f607ebb6115a6000d7a1751d))

# [1.4.0](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.13...v1.4.0) (2024-02-27)


### Bug Fixes

* change materialized views to views ([15b1b2e](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/15b1b2e34f7aae5c2a67ebca0b03e727fa3522a7))
* remove suspicious case ([e7fbd7a](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e7fbd7ac4ca08a4b6e04e351771f1e99b4ca76c8))


### Features

* add descriptions and fullname ([92a4c46](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/92a4c469dd2f962e489746e98cdc0a22bd24d816))
* add ProposedAddressWithoutLinkedParcelOrBuildingUnits view ([69a5d87](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/69a5d8769c11c5b9a39d146f3832757b79593d1b))
* add view ActiveBuildingUnitLinkedToMultipleAddresses ([e32761c](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e32761ca26d57ce27c6c8f690efee40dea4a0760))
* add view ActiveBuildingUnitWithoutAddress ([82abb79](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/82abb7959ab938cfc582d65837dae9cd9e1cf811))
* add view AddressesLinkedToMultipleBuildingUnits ([a2c2af3](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/a2c2af39f7a4d96f6a4c09dad1fac288bd57eca1))
* add view AddressesLongerThanTwoYearsProposed ([1daca8e](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/1daca8e6055349aedca739ce0ddc86572c43895c))
* add view BuildingsLongerThanTwoYearsPlanned ([6e58ed8](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/6e58ed81e7e912fe10484ed2d2f35699157e4c97))
* add view BuildingUnitsLongerThanTwoYearsPlanned ([ed39e41](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/ed39e414fa2401564acffc5ac54a06710f9a89e4))
* add view CurrentAddressesOutsideOfMunicipalityBounds ([f974492](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/f974492daf4c9598d6112fa70502a4e74d7827a0))
* add view CurrentAddressesWithSpecificationDerivedFromObjectWithoutBuildingUnit ([e2c0e32](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e2c0e3289337dc34c27ed0e56e8fec9c9d72645d))
* add view CurrentAddressWithoutLinkedParcelOrBuildingUnit ([0b5253c](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/0b5253c154b4ff6a8b1237dd0dfde28ef356752e))
* add view CurrentStreetNameWithoutLinkedRoadSegments ([e93bac7](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e93bac7b3b43180994087a1ea099906a66f5ded6))
* add view StreetNameWithOnlyOneRoadSegmentToOnlyOneSide ([6a933ab](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/6a933abcf1ff6199e9558f3e473da75fc8df1d21))

## [1.3.13](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.12...v1.3.13) (2024-02-20)


### Bug Fixes

* build suspicious api image ([aea9dea](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/aea9dea1dd51536f91105c56e3a6df52523371e4))

## [1.3.12](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.11...v1.3.12) (2024-02-20)


### Bug Fixes

* update build suspicious cases api ([9e57961](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/9e57961899fffa34999dfc1e5e045fd88c8da4a3))

## [1.3.11](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.10...v1.3.11) (2024-02-19)


### Bug Fixes

* image name suspicious cases api ([0078389](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/0078389d6d4a23e1e50cc40c8be1a50f33b3e75d))

## [1.3.10](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.9...v1.3.10) (2024-02-19)


### Bug Fixes

* bump to trigger build ([ebd5b9d](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/ebd5b9dfc129d087332fe5bd8641f5cc3b6b3c4d))

## [1.3.9](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.8...v1.3.9) (2024-02-07)


### Bug Fixes

* **bump:** use new deploy pipeline ([dd07546](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/dd07546cedf154622c88127d45be76645cf45d88))

## [1.3.8](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.7...v1.3.8) (2024-02-05)


### Bug Fixes

* **bump:** ci new pipeline structure ([81495d3](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/81495d328a157af72a0561b1ab4e72d3f49eab2b))

## [1.3.7](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.6...v1.3.7) (2024-02-05)


### Bug Fixes

* set last event position ([ca1a74d](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/ca1a74daf8259c431bd53b20aae5fd2c5c48cdca))

## [1.3.6](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.5...v1.3.6) (2024-01-31)


### Bug Fixes

* don't parse empty string to datetime ([e211cca](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e211ccaea2acbb0725687feea451c021e17eddbd))

## [1.3.5](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.4...v1.3.5) (2024-01-30)


### Bug Fixes

* docs listitem collection ([9360b37](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/9360b3716478a6c878ed60d4a93284355474095d))
* indieningsdatum should only show date ([bfb8e4b](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/bfb8e4b0b56d067cd47dd69518f9261dd03892ff))
* **suspicious:** docs detail ([ace966e](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/ace966e8bf7684d0ea4697252ed44e92478ac106))
* **suspicious:** docs listresponseitem ([2bfd777](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/2bfd7771c8599df176d1d377bf349e89ba100bbd))

## [1.3.4](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.3...v1.3.4) (2024-01-30)


### Bug Fixes

* **bump:** ci add image to production ([e844123](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e844123292a265043c5c82e381d43d4cc0d91cef))

## [1.3.3](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.2...v1.3.3) (2024-01-30)


### Bug Fixes

* next url ([206a9b6](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/206a9b6a0dfe3e445ae96ea6842e149ac44c188f))

## [1.3.2](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.1...v1.3.2) (2024-01-30)


### Bug Fixes

* documentation examples ([13a02df](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/13a02df4c6ae7407f12f16ec2634b1d42125c1eb))

## [1.3.1](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.3.0...v1.3.1) (2024-01-29)


### Bug Fixes

* refine logging ([0cec930](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/0cec9309187407686974a3963dcfb53d465c33a1))

# [1.3.0](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.2.2...v1.3.0) (2024-01-29)


### Bug Fixes

* null ref filter sus piciouscases ([04fcd64](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/04fcd6406b4a15b2a9a1ac62160c9f4a92d0a700))


### Features

* add dynamo ([da045de](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/da045dea600c047dc617adea88f1a6794704c35a))
* add email sender ([6925780](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/6925780dd053334141b77f2f4a98d98925d7bb7c))
* add gtmf integration VEKA ([e8a2b36](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e8a2b36228447ce74bdc24ea506c35e85282ff95))
* add nationalregistry skeleton ([9122e02](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/9122e02165be6296681d380332af5e1e0dc6b17c))
* add notification service ([fa46c5a](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/fa46c5a393da7c29fe05b826f2af359b104d55e8))
* loop events ([08d5f65](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/08d5f65901ed2d08392c2c3630c9bd644feaef7e))

## [1.2.2](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.2.1...v1.2.2) (2024-01-25)


### Bug Fixes

* move supsicious cases filters to abstractions ([dd5644d](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/dd5644dd0ef6b7fb206a97d78545822f716462f5))

## [1.2.1](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.2.0...v1.2.1) (2024-01-24)


### Bug Fixes

* push images ([b5a419c](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/b5a419c7fec44bda8b37b3ae078e8571f5709162))

# [1.2.0](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.1.2...v1.2.0) (2024-01-24)


### Features

* add abstractions ([631dc42](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/631dc42bbeafcffdbfa34d41afd44afe2987ecf7))

## [1.1.2](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.1.1...v1.1.2) (2024-01-24)


### Bug Fixes

* try new paket template for api ([d3f3e0e](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/d3f3e0e7dc9c90f4d962c1c40a365457d75b0002))

## [1.1.1](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.1.0...v1.1.1) (2024-01-23)


### Bug Fixes

* add code comment docs per endpoint ([dcc1eee](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/dcc1eeef5d3d7b8f55078c6d15a5b65ce126cb5f))

# [1.1.0](https://github.com/informatievlaanderen/basisregisters-integration-db/compare/v1.0.0...v1.1.0) (2024-01-23)


### Bug Fixes

* add swagger docs ([5b2cff2](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/5b2cff2bedc0a2381d317c4c01d7af597d769512))


### Features

* add gtmf integration skeleton ([3f9f247](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/3f9f247b43fdeef936e55d10f2b55da6a094485e))

# 1.0.0 (2024-01-22)


### Bug Fixes

* finetune schemas and add geometry computed column ([61fb515](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/61fb5155522a3c1c92c370ca1de9df52531dfcd7))
* importer to use lambert31370 ([dccbea1](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/dccbea1ab2f3633f992cf746d095b88949fa1d40))
* migrations + views ([e9e7217](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e9e7217ca5378c88739a05e3041bd9423a3cef6d))
* rename project/solution ([57e7a3e](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/57e7a3e388fd8ffddf0156209708f56c2be88c94))


### Features

* add consumers, add migrations for tables, views, indexes ([5278666](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/52786660bb15f2e1968b33f59e463f7eeee992df))
* add idempotencekeys to tables ([0acb2f9](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/0acb2f9c920cdf997741d8f0af56d6fff2d2cc76))
* add municipality geometry importer ([e7f82c9](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/e7f82c9d5ac1c8fcb77018cf22b802431b557b54))
* add suspicious cases api skeleton ([712e424](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/712e4248c3acd6addb5ddc3f48cc0a485d597adc))
* initial ([570db5b](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/570db5b78ae985174799bf2b9a932f3a36566bf0))
* niscode auth filtering ([c9757ff](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/c9757fffc4e33f252e50b19727d56b6e79da7b89))
* rename project and create StreetNamesLongerThanTwoYearsProposed ([da61005](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/da6100544154a030fd3131f290a72831c97a26d2))
* suspicious case - address w no attached parcel or buildingUnit ([3ed93b1](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/3ed93b1d22321bcfc987cae43c021e112a441582))
* suspicious cases detail contracts ([3360ebd](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/3360ebd22d68364cfe76b38f397cf3baaf3b785d))
* suspicious cases list contracts ([498b098](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/498b098176771bf73f230ae3df74d8914cb3a16a))
* suspicious cases list queries ([4928a08](https://github.com/informatievlaanderen/basisregisters-integration-db/commit/4928a08c058597744d31ed3a30d046a7ba95fe1e))
