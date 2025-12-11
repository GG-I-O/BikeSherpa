// import Interceptors from "./interceptors/Interceptors";
import axios from "axios";
import IOCContainerBootstrapper from "./IOCContainerBootstrapper";

export default class AppBootstrapper {

    public static init() {
        IOCContainerBootstrapper.init();

        axios.defaults.baseURL = process.env.EXPO_PUBLIC_API_URL;

        // const interceptors = new Interceptors();
        // interceptors.startToIntercept();
    }
}