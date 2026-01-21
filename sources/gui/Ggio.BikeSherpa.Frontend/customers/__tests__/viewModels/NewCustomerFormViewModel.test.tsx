import Customer from "@/customers/models/Customer";
import InputCustomer from "@/customers/models/InputCustomer";
import NewCustomerFormViewModel from "@/customers/viewModels/NewCustomerFormViewModel";
import { ICustomerService } from "@/spi/CustomerSPI";
import { mock } from "ts-jest-mocker";

const customerService = mock<ICustomerService>();

describe("NewCustomerFormViewModel", () => {
    const mockCustomer = new InputCustomer(
        "Test Customer",
        { name: "", fullAddress: "123 Test St", streetInfo: "123 Test St", postcode: "12345", city: "Test City" },
        "TEST001",
        "1234567890",
        "test@example.com"
    );
    customerService.createCustomer = jest.fn();
    const viewModel = new NewCustomerFormViewModel(customerService);

    beforeEach(() => {
        jest.clearAllMocks();
    })

    it("onSubmit calls createCustomer with correct customer", () => {
        viewModel.onSubmit(mockCustomer);
        mockCustomer.address.name = mockCustomer.name;
        expect(customerService.createCustomer).toHaveBeenCalledTimes(1);
        expect(customerService.createCustomer).toHaveBeenCalledWith(mockCustomer);
    })

    it("onSubmit calls createCustomer with correct customer", () => {
        const mockResetCallback = jest.fn();
        viewModel.setResetCallback(mockResetCallback);
        viewModel.onSubmit(mockCustomer);

        expect(customerService.createCustomer).toHaveBeenCalledTimes(1);
        expect(mockResetCallback).toHaveBeenCalledTimes(1);
    })

    describe("getNewCustomerSchema", () => {
        const existingCustomers = [
            new Customer("1", { name: "Existing Customer", fullAddress: "", streetInfo: "", postcode: "", city: "" }, "EXI", "1111111111", "existing@test.com")
        ];


        let customerToValidate: Customer;

        beforeEach(() => {
            customerToValidate = new Customer(
                "Company",
                { name: "New Customer", fullAddress: "10 rue de la Paix 75000 Paris", streetInfo: "10 rue de la Paix", postcode: "7500", city: "Paris" },
                "EST",
                "0606060606",
                "new@test.com");
        })

        it("validates name is required", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.name = "";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Nom requis");
            }
        });

        it("validates code is required", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.code = "";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code requis");
            }
        });

        it("validates code max length", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.code = "ESTEST";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code trop long");
            }
        });

        it("validates code uniqueness", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.code = "EXI";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Le code doit être unique");
            }
        });

        it("validates email format", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.email = "invalid.email";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Adresse e-mail non valide");
            }
        });

        it("accepts valid customer data", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(true);
        });
    });
})