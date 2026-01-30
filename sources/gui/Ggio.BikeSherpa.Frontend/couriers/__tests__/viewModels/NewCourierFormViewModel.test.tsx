import Courier from "@/couriers/models/Courier";
import NewCourierFormViewModel from "@/couriers/viewModels/NewCourierFormViewModel";
import { createRandomCourier, createRandomInputCourier, linkType } from "@/fixtures/courier-fixtures";
import { ICourierService } from "@/spi/CourierSPI";
import { faker } from "@faker-js/faker";
import { mock } from "ts-jest-mocker";

const courierService = mock<ICourierService>();

describe("NewCourierFormViewModel", () => {
    const mockCourier = createRandomInputCourier();
    courierService.createCourier = jest.fn();
    const viewModel = new NewCourierFormViewModel(courierService);

    beforeEach(() => {
        jest.clearAllMocks();
    })

    it("onSubmit calls createCourier with correct courier", () => {
        viewModel.onSubmit(mockCourier);
        mockCourier.address.name = `${mockCourier.firstName} ${mockCourier.lastName}`;
        expect(courierService.createCourier).toHaveBeenCalledTimes(1);
        expect(courierService.createCourier).toHaveBeenCalledWith(mockCourier);
    })

    it("onSubmit calls createCourier with correct courier", () => {
        const mockResetCallback = jest.fn();
        viewModel.setResetCallback(mockResetCallback);
        viewModel.onSubmit(mockCourier);

        expect(courierService.createCourier).toHaveBeenCalledTimes(1);
        expect(mockResetCallback).toHaveBeenCalledTimes(1);
    })

    describe("getNewCourierSchema", () => {
        const existingCouriers: Courier[] = faker.helpers.multiple(() => createRandomCourier(true, linkType.none), {
            count: 1
        });


        let courierToValidate: Courier;

        beforeEach(() => {
            courierToValidate = createRandomCourier(true, linkType.none);
        })

        it("validates firstName is required", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.firstName = "";

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Prénom requis");
            }
        });

        it("validates lastName is required", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.lastName = "";

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Nom requis");
            }
        });

        it("validates email is required", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.email = "";

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Adresse e-mail requise");
            }
        });

        it("validates phone is required", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.phoneNumber = "";

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Numéro de téléphone requis");
            }
        });

        it("validates code is required", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.code = "";

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code requis");
            }
        });

        it("validates code max length", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.code = "ESTEST";

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code trop long");
            }
        });

        it("validates code uniqueness", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.code = existingCouriers[0].code;

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Le code doit être unique");
            }
        });

        it("validates email format", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.email = "invalid.email";

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Adresse e-mail invalide");
            }
        });

        it("validates phone format", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);
            courierToValidate.phoneNumber = "invalid.phone.number";

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Numéro de téléphone invalide");
            }
        });

        it("accepts valid courier data", () => {
            //arrange
            const schema = viewModel.getNewCourierSchema(existingCouriers);

            //act
            const result = schema.safeParse(courierToValidate);

            //assert
            expect(result.success).toBe(true);
        });
    });
})