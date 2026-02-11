import Courier from "@/couriers/models/Courier";
import InputCourier from "@/couriers/models/InputCourier";
import CourierServices from "@/couriers/services/CourierServices";
import { createRandomCourier, createRandomInputCourier, linkType } from "@/fixtures/courier-fixtures";
import { ICourierService } from "@/spi/CourierSPI";
import { ILogger } from "@/spi/LogsSPI";
import { IStorageContext } from "@/spi/StorageSPI";
import { faker } from "@faker-js/faker";
import { Observable, observable } from "@legendapp/state";
import * as Crypto from "expo-crypto";
import { mock } from "ts-jest-mocker";

const logger = mock<ILogger>();
const storage = mock<IStorageContext<Courier>>();

describe("CourierServices", () => {
    let mockCourierStore$: Observable<Record<string, Courier>>;
    let courierService: ICourierService;
    let mockCouriers: Courier[];
    beforeEach(() => {
        mockCouriers = faker.helpers.multiple(() => createRandomCourier(true, linkType.none), {
            count: 2,
        });
        mockCourierStore$ = observable<Record<string, Courier>>({
            [mockCouriers[0].id]: mockCouriers[0],
            [mockCouriers[1].id]: mockCouriers[1]
        });
        storage.getStore.mockReturnValue(mockCourierStore$);
        logger.extend.mockReturnValue(logger);
        courierService = new CourierServices(logger, storage);
    });

    it("getCourierList$ returns a courier list", () => {
        //arrange

        //act
        const courierList = courierService.getCourierList$();

        //assert
        expect(courierList).not.toBeNull();
        expect(courierList).toBe(mockCourierStore$);
    })

    it("getCourier$ returns a courier", () => {
        //arrange

        //act
        const courier = courierService.getCourier$(mockCouriers[0].id);

        //assert
        expect(courier).not.toBeNull();
        expect(courier).toBe(mockCourierStore$[mockCouriers[0].id]);
    })

    it("deleteCourier throws an error when no link is present", () => {
        //arrange

        //act

        //assert
        expect(() => courierService.deleteCourier(mockCouriers[0].id)).toThrow(`Cannot delete the courier ${mockCouriers[0].id}`);
    })

    it("deleteCourier deletes a courier when links exist", () => {
        //arrange
        mockCourierStore$[mockCouriers[0].id].links.set([
            {
                rel: "delete",
                href: `/api/couriers/${mockCouriers[0].id}`,
                method: "DELETE"
            }
        ])

        //act
        courierService.deleteCourier(mockCouriers[0].id);

        //assert
        expect(mockCourierStore$.peek()[mockCouriers[0].id]).toBeUndefined();
    })

    it("createCourier creates a courier", () => {
        //arrange
        const newCourier: InputCourier = createRandomInputCourier();
        jest.spyOn(Crypto, 'randomUUID').mockReturnValue("789");

        //act
        courierService.createCourier(newCourier);

        //assert
        const newCourierPeek = mockCourierStore$.peek()["789"];
        expect(newCourierPeek).not.toBeUndefined();
        expect(newCourierPeek.firstName).toBe(newCourier.firstName);
        expect(newCourierPeek.code).toBe(newCourier.code);
    })

    it("updateCourier updates a courier when links exist", () => {
        //arrange
        mockCourierStore$[mockCouriers[0].id].links.set([
            {
                rel: "update",
                href: `/api/couriers/${mockCouriers[0].id}`,
                method: "PUT"
            }
        ])

        const courierToUpdate: Courier = {
            ...createRandomCourier(true, linkType.none),
            id: mockCouriers[0].id
        };

        //act
        courierService.updateCourier(courierToUpdate);
        const updatedCourierPeek = mockCourierStore$.peek()[mockCouriers[0].id];

        //assert
        expect(updatedCourierPeek.lastName).toBe(courierToUpdate.lastName);
        expect(updatedCourierPeek.code).toBe(courierToUpdate.code);
    })

    it("updateCourier does not update a courier when there is no link", () => {
        //arrange
        const courierToUpdate: Courier = {
            ...createRandomCourier(true, linkType.none),
            id: mockCouriers[0].id
        };
        //act

        //assert
        expect(() => courierService.updateCourier(courierToUpdate)).toThrow(`Cannot update courier ${courierToUpdate.id}`);
    })
})