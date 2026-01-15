import Customer from "@/customers/models/Customer";
import InputCustomer from "@/customers/models/InputCustomer";
import EditCustomerFormViewModel from "@/customers/viewModels/EditCustomerFormViewModel";
import { ICustomerService } from "@/spi/CustomerSPI";
import { mock } from "ts-jest-mocker";

const customerService = mock<ICustomerService>();

describe("NewCustomerFormViewModel", () => {
    const mockCustomer = new Customer(
        "Test Customer",
        { name: "", fullAddress: "123 Test St", streetInfo: "123 Test St", postcode: "12345", city: "Test City" },
        "TEST001",
        "1234567890",
        "test@example.com"
    );
    customerService.updateCustomer = jest.fn();
    const viewModel = new EditCustomerFormViewModel(customerService);

    beforeEach(() => {
        jest.clearAllMocks();
    })

    it("onSubmit calls updateCustomer with correct customer", () => {
        viewModel.onSubmit(mockCustomer);
        mockCustomer.address.name = mockCustomer.name;
        expect(customerService.updateCustomer).toHaveBeenCalledTimes(1);
        expect(customerService.updateCustomer).toHaveBeenCalledWith(mockCustomer);
    })

    describe("getEditCustomerSchema", () => {
        const existingCustomers: Customer[] = [
            {
                id: "123",
                name: "Existing Customer",
                address: {
                    name: "Existing Customer",
                    fullAddress: "",
                    streetInfo: "",
                    complement: undefined,
                    postcode: "",
                    city: ""
                },
                code: "EXI",
                phoneNumber: "0609080704",
                email: "existing.customer@gmail.com"
            }
        ];

        let customerToEdit: Customer;

        beforeEach(() => {
            customerToEdit = {
                id: "456",
                name: "Existing Company",
                address: {
                    name: "Existing Company",
                    fullAddress: "10 rue de la Paix 75000 Paris",
                    streetInfo: "10 rue de la Paix",
                    complement: undefined,
                    postcode: "75000",
                    city: "Paris"
                },
                code: "EDI",
                phoneNumber: "0609080704",
                email: "existing.company@gmail.com"
            }
        })

        it("validates name is required", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.name = "";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Nom requis");
            }
        });

        it("validates code is required", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.code = "";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code requis");
            }
        });

        it("validates code max length", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.code = "ESTEST";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code trop long");
            }
        });

        it("validates code uniqueness", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.code = "EXI";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Le code doit être unique");
            }
        });

        it("validates email format", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.email = "invalid.email";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Adresse e-mail non valide");
            }
        });

        it("accepts valid customer data", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(true);
        });
    });
})